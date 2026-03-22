provider "aws" {
  region = var.region
}

module "eks" {
  source  = "terraform-aws-modules/eks/aws"
  version = "19.16.0"

  name    = var.cluster_name
  version = "1.27"

  # Criar VPC e subnets automaticamente
  vpc_id     = null
  subnet_ids = null

  # Node group
  node_groups = {
    oficina_nodes = {
      desired_capacity = var.desired_capacity
      max_capacity     = var.max_capacity
      min_capacity     = var.min_capacity
      instance_type    = var.node_instance_type

      additional_iam_policies = [
        "arn:aws:iam::aws:policy/AmazonEKSWorkerNodePolicy",
        "arn:aws:iam::aws:policy/AmazonEC2ContainerRegistryReadOnly",
        "arn:aws:iam::aws:policy/AmazonEKS_CNI_Policy"
      ]
    }
  }

  # Políticas IAM adicionais do cluster
  cluster_iam_role_additional_policies = [
    "arn:aws:iam::aws:policy/AmazonEKSClusterPolicy"
  ]

  # Autorizações de autenticação
  manage_aws_auth = true
}