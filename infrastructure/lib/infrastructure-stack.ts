import * as cdk from 'aws-cdk-lib';
import { Construct } from 'constructs';

export class InfrastructureStack extends cdk.Stack {
  public readonly repository: cdk.aws_ecr.Repository;

  constructor(scope: Construct, id: string, props?: cdk.StackProps) {
    super(scope, id, props);

    this.repository = new cdk.aws_ecr.Repository(this, 'Repository', {
      removalPolicy: cdk.RemovalPolicy.DESTROY,
      emptyOnDelete: true,
    });
  }
}