# Stage build: dùng Node.js để build frontend
FROM node:20-alpine AS build
WORKDIR /app

# Copy package.json và cài dependencies
COPY package*.json ./
RUN npm install

# Copy toàn bộ source code và build
COPY . .
RUN npm run build -- --configuration production

# Copy file nginx.conf vào stage build
COPY frontend/nginx.conf /app/nginx.conf

# Stage final: dùng Nginx để serve app
FROM nginx:alpine

# Copy thư mục build frontend từ stage build
COPY --from=build /app/dist/frontend/browser /usr/share/nginx/html

# Copy file nginx.conf từ stage build
COPY --from=build /app/nginx.conf /etc/nginx/conf.d/default.conf

# Expose port 80
EXPOSE 80

# Chạy nginx
CMD ["nginx", "-g", "daemon off;"]
