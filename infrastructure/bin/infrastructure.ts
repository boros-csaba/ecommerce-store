#!/usr/bin/env node
import 'source-map-support/register';
import * as cdk from 'aws-cdk-lib';
import { InfrastructureStack } from '../lib/infrastructure-stack';
import { DemoStack } from '../lib/demo-stack';

const app = new cdk.App();
cdk.Tags.of(app).add('project', 'elenora');

const env = {
  region: 'eu-central-1',
  account: process.env.CDK_DEFAULT_ACCOUNT,
}

new InfrastructureStack(app, 'Elenora-Infrastructure', { env });
new DemoStack(app, 'Elenora-Demo', { env });