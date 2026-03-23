output "cluster_id" {
  value = module.eks-cluster.cluster_id
}

output "cluster_endpoint" {
  value = module.eks-cluster.cluster_endpoint
}

output "cluster_certificate_authority_data" {
  value = module.eks-cluster.cluster_certificate_authority_data
}

output "node_group_names" {
  value = keys(module.eks-cluster.eks_managed_node_groups)
}