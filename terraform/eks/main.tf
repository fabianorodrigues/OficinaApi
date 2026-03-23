provider "aws" {
  region = var.region
}

module "eks" {
  source  = "terraform-aws-modules/eks/aws"
  version = "~> 20.0" # Versão estável recomendada

  cluster_name    = var.cluster_name
  cluster_version = var.cluster_version

  vpc_id     = var.vpc_id
  subnet_ids = var.subnet_ids

  # Cluster IAM Role - Usando role existente
  create_iam_role          = false
  iam_role_arn             = var.cluster_iam_role_arn

  # Acesso ao cluster
  enable_cluster_creator_admin_permissions = true

  eks_managed_node_groups = {
    oficina_nodes = {
      instance_types = var.node_instance_types
      min_size     = var.min_capacity
      max_size     = var.max_capacity
      desired_size = var.desired_capacity

      create_iam_role = false
      iam_role_arn    = var.node_iam_role_arn

      create_iam_role_policy = false
    }
  }
}