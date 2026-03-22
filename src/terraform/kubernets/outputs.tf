output "namespace" {
  value = kubernetes_namespace.oficina.metadata[0].name
}

output "deployment_name" {
  value = kubernetes_deployment.oficina_app.metadata[0].name
}

output "service_name" {
  value = kubernetes_service.oficina_service.metadata[0].name
}

output "service_port" {
  value = kubernetes_service.oficina_service.spec[0].port[0].port
}

output "docker_image" {
  value = "${var.docker_image_repo}:${var.docker_image_tag}"
}