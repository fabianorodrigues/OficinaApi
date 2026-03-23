variable "namespace_name" {
  type    = string
  default = "oficina"
}

variable "docker_image_repo" {
  type    = string
  default = "jaquelineramosit/oficina-api"
}

variable "docker_image_tag" {
  description = "Tag da imagem Docker (via GitHub Actions)"
  type        = string
  default     = "latest"
}

variable "replicas" {
  type    = number
  default = 1
}

variable "container_port" {
  type    = number
  default = 8080
}

variable "service_port" {
  type    = number
  default = 80
}