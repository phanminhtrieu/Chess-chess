# Stage 1: Build frontend
FROM node:20-alpine AS build
WORKDIR /app

COPY frontend/package*.json ./
RUN npm install

COPY frontend/ ./
RUN npm run build -- --configuration production

# Copy nginx.conf vào stage build
COPY frontend/nginx.conf /app/nginx.conf

# Stage 2: Serve with Nginx
FROM nginx:alpine

# Copy build output
COPY --from=build /app/dist/browser /usr/share/nginx/html

# Copy nginx.conf từ stage build
COPY --from=build /app/nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]# Stage 1: Build frontend
FROM node:20-alpine AS build
WORKDIR /app

COPY frontend/package*.json ./
RUN npm install

COPY frontend/ ./
RUN npm run build -- --configuration production

# Stage 2: Serve with Nginx
FROM nginx:alpine

# Copy build output
COPY --from=build /app/dist/browser /usr/share/nginx/html

# Copy custom nginx config
COPY frontend/nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
