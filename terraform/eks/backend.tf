terraform {
  backend "s3" {
    bucket = "tf-backend-oficina-api"
    key = "fiap/terraform.tfstate"
    region = "us-east-1"
  }
}