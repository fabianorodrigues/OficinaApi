provider "aws" {
  region = var.region
}

module "eks" {
  source  = "terraform-aws-modules/eks/aws"
  version = "~> 21.0"

  name               = var.cluster_name
  kubernetes_version = var.cluster_version

  vpc_id     = var.vpc_id
  subnet_ids = var.subnet_ids

  # Role existente do cluster EKS
  cluster_iam_role_name = var.cluster_iam_role_name

  # Node group gerenciado usando role existente
  eks_managed_node_groups = {
    oficina_nodes = {
      instance_types     = var.node_instance_types
      min_size           = var.min_capacity
      max_size           = var.max_capacity
      desired_size       = var.desired_capacity
      node_iam_role_name = var.node_iam_role_name
    }
  }

  # Concede permissões de admin no kubeconfig automaticamente
  enable_cluster_creator_admin_permissions = true
}