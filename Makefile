TAG ?= 2025

deploy:
	cd api2025 && docker buildx build --platform linux/amd64,linux/arm64 -t tuso/hackyeah-api:${TAG} --push .

.PHONY: deploy
