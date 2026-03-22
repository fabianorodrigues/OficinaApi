provider "kubernetes" {
  config_path = "~/.kube/config"
}

resource "kubernetes_namespace" "oficina" {
  metadata {
    name = var.namespace_name
  }
}