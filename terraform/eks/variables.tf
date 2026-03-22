variable "region" {
  description = "Região AWS para criar o cluster"
  type        = string
  default     = "us-east-1"
}

variable "cluster_name" {
  description = "Nome do cluster EKS"
  type        = string
  default     = "oficina-cluster"
}

variable "node_instance_type" {
  description = "Tipo de instância EC2 para os nodes"
  type        = string
  default     = "t3.medium"
}

variable "desired_capacity" {
  description = "Número de nodes desejado"
  type        = number
  default     = 1
}

variable "max_capacity" {
  description = "Máximo de nodes no node group"
  type        = number
  default     = 2
}

variable "min_capacity" {
  description = "Mínimo de nodes no node group"
  type        = number
  default     = 1
}