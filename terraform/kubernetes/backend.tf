terraform {
  backend "s3" {
    bucket = "tf-backend-oficina-api"
    key = "k8s/terraform.tfstate"
    region = "us-east-1"
  }
}