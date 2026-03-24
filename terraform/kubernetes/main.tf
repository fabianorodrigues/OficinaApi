# --- PROVIDER E DATAS (Igual ao seu) ---
data "aws_eks_cluster" "cluster" {
  name = "EKS-Oficina"
}

data "aws_eks_cluster_auth" "cluster" {
  name = "EKS-Oficina"
}

provider "kubernetes" {
  host                   = data.aws_eks_cluster.cluster.endpoint
  cluster_ca_certificate = base64decode(data.aws_eks_cluster.cluster.certificate_authority[0].data)
  token                  = data.aws_eks_cluster_auth.cluster.token
}

# --- 1. NAMESPACE ---
resource "kubernetes_namespace_v1" "oficina" {
  metadata {
    name = var.namespace_name
  }
}

# --- 2. CONFIGMAP ---
resource "kubernetes_config_map_v1" "oficina_config" {
  metadata {
    name      = "oficina-config"
    namespace = kubernetes_namespace_v1.oficina.metadata[0].name
  }
  data = {
    ASPNETCORE_ENVIRONMENT = "Development"
  }
}

# --- 3. SECRET ---
resource "kubernetes_secret_v1" "oficina_secret" {
  metadata {
    name      = "oficina-secret"
    namespace = kubernetes_namespace_v1.oficina.metadata[0].name
  }
  type = "Opaque"
  data = {
    CONNECTION_STRING = "Server=sqlserver,1433;Database=OficinaDb;User Id=sa;Password=Your_password123!;TrustServerCertificate=True;"
  }
}

# --- 4. DEPLOYMENT ---
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
          
          # Injeta todas as chaves do ConfigMap e da Secret como variáveis de ambiente
          env_from {
            config_map_ref {
              name = kubernetes_config_map_v1.oficina_config.metadata[0].name
            }
          }
          env_from {
            secret_ref {
              name = kubernetes_secret_v1.oficina_secret.metadata[0].name
            }
          }

          # Mapeamento para o nome exato que o seu código .NET espera
          env {
            name = "ConnectionStrings__SqlServer" 
            value_from {
              secret_key_ref {
                name = kubernetes_secret_v1.oficina_secret.metadata[0].name
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

# --- 5. SERVICE ----
resource "kubernetes_service_v1" "oficina_service" { 
  metadata {
    name      = "oficina-service"
    namespace = kubernetes_namespace_v1.oficina.metadata[0].name
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