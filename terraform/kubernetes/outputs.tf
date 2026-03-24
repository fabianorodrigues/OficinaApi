output "namespace" {
  value = kubernetes_namespace_v1.oficina.metadata[0].name
}

output "deployment_name" {
  value = kubernetes_deployment_v1.oficina_app.metadata[0].name
}

output "service_name" {
  value = kubernetes_service_v1.oficina_service.metadata[0].name
}

output "docker_image" {
  value = "${var.docker_image_repo}:${var.docker_image_tag}"
}