terraform {
  backend "s3" {
    bucket = "tf-backend-oficina-api"
    key = "eks/terraform.tfstate"
    region = "us-east-1"
  }
}