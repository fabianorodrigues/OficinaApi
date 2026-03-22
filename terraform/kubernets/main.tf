provider "kubernetes" {
  config_path = "" # Vai usar KUBECONFIG do ambiente
}

resource "kubernetes_namespace" "oficina" {
  metadata {
    name = var.namespace_name
  }
}

resource "kubernetes_deployment" "oficina_app" {
  metadata {
    name      = "oficina-app"
    namespace = kubernetes_namespace.oficina.metadata[0].name
  }

  spec {
    replicas = var.replicas

    selector {
      match_labels = {
        app = "oficina-app"
      }
    }

    template {
      metadata {
        labels = {
          app = "oficina-app"
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
    namespace = kubernetes_namespace.oficina.metadata[0].name
  }

  spec {
    selector = {
      app = kubernetes_deployment.oficina_app.metadata[0].labels["app"]
    }

    port {
      port        = var.service_port
      target_port = var.container_port
    }

    type = "LoadBalancer"
  }
}