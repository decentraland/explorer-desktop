# Encrypt file

To upload a new AVPro package to the CI, we need to encrypt it with gpg to later upload it to an S3 bucket. Its recommended to complete this steps on a Linux console.


## 1. Generate key
```sh
gpg --quick-generate-key dcl
```

## 2. Encrypt file
```sh
gpg --output filename.gpg --encrypt --recipient dcl filename
```

## 3. Export private key (to decrypt)

```sh
gpg --armor --export-secret-key dcl | base64 -w 0
```

Replace the private key into the Environment variables of the CI. Use the name GPG_PRIVATE_KEY_BASE64; and set the value to the private key.

## 4. Upload package to S3 Bucket

Ask someone with access to S3 to replace the current package with the new one. (for example, Mateo Miccino)

## 5. Rename files in ci-build.sh 

If necessary, rename the files in the ci-build.sh file


## Steps not necessary since we are manually uploading files to the CI

## Import private key (from CI)

```sh
echo ${KEY} | base64 -d > private.gpg
gpg --import private.gpg
```

## Decrypt file (from CI)

```sh
gpg --output filename --decrypt filename.gpg
```