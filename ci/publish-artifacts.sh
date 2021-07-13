#!/bin/bash

######################################################
# This file deploys the ./dist folder to the S3 bucket
# and creates an invalidation in the cloudfront caché
######################################################

set -u # no unbound variables

# Upload artifacts
aws s3 sync /tmp/workspace/explorer-desktop/unity-desktop-artifacts/ "s3://${S3_BUCKET}/desktop/${CIRCLE_BRANCH}" --acl public-read

# Invalidate cache
aws configure set preview.cloudfront true
aws configure set preview.create-invalidation true
aws cloudfront create-invalidation --distribution-id "${CLOUDFRONT_DISTRIBUTION}" --paths "/desktop/${CIRCLE_BRANCH}/*"