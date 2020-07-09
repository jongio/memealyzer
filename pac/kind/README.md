# Local Kind Cluster Configuration

1. Create a kind cluster with the configuration in `config.yaml`:

```sh
kind create cluster --config ./config.yaml
```

`config.yaml` contents:

```yaml
kind: Cluster
    apiVersion: kind.x-k8s.io/v1alpha4
    nodes:
    - role: control-plane
      kubeadmConfigPatches:
      - |
        kind: InitConfiguration
        nodeRegistration:
          kubeletExtraArgs:
            node-labels: "ingress-ready=true"
      extraPortMappings:
      - containerPort: 80
        hostPort: 8888
        protocol: TCP
    - role: worker
```

2. This configuration maps the control-plane node port 80 to your host's 8888, making it easy to access the NodePort service that will be created by the nginx deployment configured for kind:

```sh
kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/master/deploy/static/provider/kind/deploy.yaml
```

You should now be able to hit the controller at http://localhost:8888

3. Load local images onto nodes:

```sh
kind load docker-image azsdkdemonetwebapp
kind load docker-image azsdkdemonetapi
kind load docker-image azsdkdemonetqueueservice
```
