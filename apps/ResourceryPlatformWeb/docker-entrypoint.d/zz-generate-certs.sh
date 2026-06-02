#!/bin/sh
set -e

CERT_DIR="/etc/nginx/certs"

if [ ! -f "$CERT_DIR/fullchain.pem" ] || [ ! -f "$CERT_DIR/privkey.pem" ]; then
  echo "[init] Generating self-signed TLS certs for development (CN=${CERT_CN:-smartserve.ecowas.int})"
  mkdir -p "$CERT_DIR" || true
  openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
    -keyout "$CERT_DIR/privkey.pem" \
    -out "$CERT_DIR/fullchain.pem" \
    -subj "/CN=${CERT_CN:-smartserve.ecowas.int}" || true
  chmod 600 "$CERT_DIR/privkey.pem" || true
else
  echo "[init] TLS certs already exist"
fi

exit 0
