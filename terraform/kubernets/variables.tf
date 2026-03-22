variable "namespace_name" {
  description = "Nome do namespace do Kubernetes"
  type        = string
  default     = "oficina"
}

variable "docker_image_repo" {
  description = "Repositório Docker sem tag"
  type        = string
  default     = "jaquelineramosit/oficina-api"
}

variable "docker_image_tag" {
  description = "Tag da imagem Docker (ex: latest ou SHA do commit)"
  type        = string
  default     = "latest"
}

variable "replicas" {
  description = "Número de réplicas do deployment"
  type        = number
  default     = 1
}

variable "container_port" {
  description = "Porta que o container expõe"
  type        = number
  default     = 8080
}

variable "service_port" {
  description = "Porta do serviço Kubernetes"
  type        = number
  default     = 80
}