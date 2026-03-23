# 1. Ajuste o Provider para buscar as credenciais do EKS na AWS
data "aws_eks_cluster" "cluster" {
  name = "EKS-Oficina" # Confirme se esse é o nome exato do seu cluster
}

data "aws_eks_cluster_auth" "cluster" {
  name = "EKS-Oficina"
}

provider "kubernetes" {
  host                   = data.aws_eks_cluster.cluster.endpoint
  cluster_ca_certificate = base64decode(data.aws_eks_cluster.cluster.certificate_authority[0].data)
  token                  = data.aws_eks_cluster_auth.cluster.token
}

# 2. Namespace
resource "kubernetes_namespace" "oficina" {
  metadata {
    name = var.namespace_name
  }
}

# 3. Deployment (Com as labels alinhadas)
resource "kubernetes_deployment" "oficina_app" {
  metadata {
    name      = "oficina-app"
    namespace = kubernetes_namespace.oficina.metadata[0].name
    labels = {
      app = "oficina-api"
    }
  }

  spec {
    replicas = var.replicas

    selector {
      match_labels = {
        app = "oficina-api"
      }
    }

    template {
      metadata {
        labels = {
          app = "oficina-api"
        }
      }

      spec {
        container {
          name  = "oficina-api"
          image = "${var.docker_image_repo}:${var.docker_image_tag}"
          port {
            container_port = var.container_port
          }
        }
      }
    }
  }
}

# 4. Service (Tipo LoadBalancer)
resource "kubernetes_service" "oficina_service" {
  metadata {
    name      = "oficina-service"
    namespace = kubernetes_namespace.oficina.metadata[0].name
  }

  spec {
    selector = {
      app = "oficina-api"
    }

    port {
      port        = var.service_port
      target_port = var.container_port
    }

    type = "LoadBalancer"
  }
}

# 5. Output da URL (Para você ver no Github Actions)
output "url_da_api" {
  value = kubernetes_service.oficina_service.status[0].load_balancer[0].ingress[0].hostname
}