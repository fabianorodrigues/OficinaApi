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

resource "kubernetes_namespace_v1" "oficina" {
  metadata {
    name = var.namespace_name
  }
}

resource "kubernetes_deployment_v1" "oficina_app" {
  metadata {
    name      = "oficina-app"
    namespace = kubernetes_namespace_v1.oficina.metadata[0].name
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

resource "kubernetes_service" "oficina_service" {
  metadata {
    name      = "oficina-service"
    namespace = kubernetes_namespace_v1.oficina.metadata[0].name
  }
  spec {
    template {
      spec {
        container {
          name  = "oficina-api"
          image = "${var.docker_image_repo}:${var.docker_image_tag}"

          env_from {
            config_map_ref {
              name = "oficina-config" # Nome do seu ConfigMap
            }
          }

          env_from {
            secret_ref {
              name = "oficina-secret" # Nome da sua Secret
            }
          }

          env {
            name = "ConnectionStrings__SqlServer" 
            value_from {
              secret_key_ref {
                name = "oficina-secret"
                key  = "CONNECTION_STRING"
              }
            }
          }

          port {
            container_port = var.container_port
          }
        }
      }
    }
  }
}