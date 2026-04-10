# Stage 1: Build
FROM node:20-alpine AS build
WORKDIR /app
COPY frontend/package*.json ./
RUN npm install
COPY frontend/ ./
RUN npm run build -- --configuration production

# Stage 2: Serve
FROM nginx:stable-alpine

# Xóa file mặc định
RUN rm -rf /usr/share/nginx/html/*

# Copy build output từ Angular
# Đảm bảo đường dẫn này khớp với folder trong dist/ sau khi build
COPY --from=build /app/dist/frontend/browser /usr/share/nginx/html

# Copy config Nginx
COPY frontend/nginx.conf /etc/nginx/conf.d/default.conf

# Cấp quyền đọc cho Nginx
RUN chmod -R 755 /usr/share/nginx/html

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
