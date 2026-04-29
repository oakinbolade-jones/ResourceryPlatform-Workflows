FROM node:20-alpine AS build
WORKDIR /app

COPY apps/ResourceryPlatformWeb/package*.json ./
RUN npm install --no-audit --no-fund

COPY apps/ResourceryPlatformWeb/ ./
RUN npm run build -- --configuration production

FROM nginx:1.27-alpine AS final
COPY apps/ResourceryPlatformWeb/nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist/ResourceryPlatformWorkflow /usr/share/nginx/html
EXPOSE 80
