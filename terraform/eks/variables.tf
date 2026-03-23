variable "region" {
  type    = string
  default = "us-east-1"
}

variable "cluster_name" {
  type    = string
  default = "oficina-cluster"
}

variable "cluster_version" {
  type    = string
  default = "1.27"
}

variable "vpc_id" {
  description = "ID da VPC onde o cluster será criado"
  type        = string
  default = "vpc-01aaf38fb64a95451"
}

variable "subnet_ids" {
  description = "Lista de subnets para o EKS"
  type        = list(string)
  default = ["subnet-0fc9572424b916ab1", "subnet-07d798dbcd128c77b", "subnet-0bf285dbdb9f928bf"]
}

variable "node_instance_types" {
  type    = list(string)
  default = ["t3.medium"]
}

variable "desired_capacity" {
  type    = number
  default = 1
}

variable "max_capacity" {
  type    = number
  default = 2
}

variable "min_capacity" {
  type    = number
  default = 1
}