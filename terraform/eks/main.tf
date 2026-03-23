module "eks" {
  source  = "terraform-aws-modules/eks/aws"
  version = "19.16.0" # fixa a versão que suporta seus argumentos

  cluster_name                   = var.cluster_name
  cluster_version                = "1.27"

  cluster_iam_role_name           = "eksClusterRole"
  node_iam_role_name              = "eksNodeRole"

  node_groups = {
    oficina_nodes = {
      desired_capacity = var.desired_capacity
      max_capacity     = var.max_capacity
      min_capacity     = var.min_capacity
      instance_type    = var.node_instance_type
      additional_iam_policies = []
    }
  }

  cluster_iam_role_additional_policies = [
    "arn:aws:iam::aws:policy/AmazonEKSClusterPolicy"
  ]

  manage_aws_auth = true

  vpc_id     = null
  subnet_ids = null
}