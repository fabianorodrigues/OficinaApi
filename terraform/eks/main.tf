provider "aws" {
  region = var.region
}

# Módulo oficial para criar EKS com Node Group e VPC
module "eks" {
  source          = "terraform-aws-modules/eks/aws"
  cluster_name    = var.cluster_name
  cluster_version = "1.27"

  # Cria VPC, subnets e SG automaticamente
  vpc_id     = null
  subnet_ids = null

  # Node group
  node_groups = {
    oficina_nodes = {
      desired_capacity = var.desired_capacity
      max_capacity     = var.max_capacity
      min_capacity     = var.min_capacity
      instance_type    = var.node_instance_type
    }
  }

  # Políticas gerenciadas do AWS para o cluster e nodes
  manage_aws_auth = true
  cluster_iam_role_name = "eks-cluster-role"
  node_iam_role_name    = "eks-node-role"

  cluster_iam_role_policy_arns = [
    "arn:aws:iam::aws:policy/AmazonEKSClusterPolicy"
  ]

  node_iam_role_policy_arns = [
    "arn:aws:iam::aws:policy/AmazonEKSWorkerNodePolicy",
    "arn:aws:iam::aws:policy/AmazonEC2ContainerRegistryReadOnly",
    "arn:aws:iam::aws:policy/AmazonEKS_CNI_Policy"
  ]
}