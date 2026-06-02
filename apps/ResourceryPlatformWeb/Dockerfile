FROM node:20-alpine AS build
WORKDIR /app

COPY apps/ResourceryPlatformWeb/package*.json ./
RUN npm install --no-audit --no-fund

COPY apps/ResourceryPlatformWeb/ ./
RUN npm run build -- --configuration production

FROM nginx:1.27-alpine AS final
RUN apk add --no-cache openssl

# Pre-generate certificates at build time
RUN mkdir -p /etc/nginx/certs && \
    openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
    -keyout /etc/nginx/certs/privkey.pem \
    -out /etc/nginx/certs/fullchain.pem \
    -subj "/CN=smartserve.ecowas.int" && \
    chmod 600 /etc/nginx/certs/privkey.pem

# Copy custom entrypoint script that will run first, before nginx default scripts
COPY apps/ResourceryPlatformWeb/docker-entrypoint.d/zz-generate-certs.sh /docker-entrypoint.d/00-generate-certs.sh
RUN chmod +x /docker-entrypoint.d/00-generate-certs.sh

# Copy nginx config
COPY apps/ResourceryPlatformWeb/nginx.conf /etc/nginx/conf.d/default.conf

COPY --from=build /app/dist/ResourceryPlatformWorkflow /usr/share/nginx/html

EXPOSE 443
