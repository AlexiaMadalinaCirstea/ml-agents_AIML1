# From https://gitlab.com/nvidia/container-images/cuda/blob/master/doc/supported-tags.md
FROM nvidia/cuda:12.6.2-cudnn-devel-ubuntu22.04

# Unminimize the system (optional)
RUN yes | unminimize

# Set the frontend to non-interactive to avoid prompts
ENV DEBIAN_FRONTEND=noninteractive

# Pre-seed the keyboard configuration to avoid interactive prompts
RUN echo "keyboard-configuration keyboard/variant select us" | debconf-set-selections \
    && echo "keyboard-configuration keyboard/layout select USA" | debconf-set-selections

# Install system dependencies
RUN apt-get update && apt-get install -y --no-install-recommends \
    wget software-properties-common curl gnupg libcurl4-openssl-dev \
    && apt-get install -y --no-install-recommends keyboard-configuration

# Add deadsnakes PPA to install Python 3.7
RUN add-apt-repository ppa:deadsnakes/ppa

# Add the Google Cloud SDK repository and GPG key using the correct keyring
RUN curl https://packages.cloud.google.com/apt/doc/apt-key.gpg | apt-key add -

# Add Google Cloud SDK repository
RUN echo "deb https://packages.cloud.google.com/apt cloud-sdk main" | tee -a /etc/apt/sources.list.d/google-cloud-sdk.list

# Install Python 3.7 and additional dependencies, including distutils for Python 3.7
RUN apt-get update && \
  apt-get install -y --no-install-recommends curl tmux vim git gdebi-core \
  build-essential python3.7 python3.7-dev python3.7-distutils python3-pip unzip google-cloud-sdk htop mesa-utils xorg-dev xorg \
  libglvnd-dev libgl1-mesa-dev libegl1-mesa-dev libgles2-mesa-dev xvfb && \
  wget http://security.ubuntu.com/ubuntu/pool/main/libx/libxfont/libxfont1_1.5.1-1ubuntu0.16.04.4_amd64.deb && \
  yes | gdebi libxfont1_1.5.1-1ubuntu0.16.04.4_amd64.deb

# Set Python 3.7 as the default Python version
RUN update-alternatives --install /usr/bin/python3 python3 /usr/bin/python3.7 1

# Upgrade pip and setuptools to the latest compatible versions
RUN python3 -m pip install --upgrade pip
RUN pip install --upgrade setuptools

# Install TensorFlow 1.15 manually (compatible with Python 3.7)
RUN pip install https://storage.googleapis.com/tensorflow/linux/cpu/tensorflow-1.15.5-cp37-cp37m-manylinux2010_x86_64.whl

# Set environment variables
ENV LD_LIBRARY_PATH=/usr/lib/x86_64-linux-gnu:$LD_LIBRARY_PATH

# Checkout ML-Agents for a specific commit (using ARG SHA)
RUN mkdir /ml-agents
WORKDIR /ml-agents
ARG SHA
RUN git init
RUN git remote add origin https://github.com/Unity-Technologies/ml-agents.git

# Retry mechanism for network issues during git fetch
RUN git fetch --depth 1 origin $SHA || (sleep 5 && git fetch --depth 1 origin $SHA)

# Checkout the fetched commit
RUN git checkout FETCH_HEAD

# Install ML-Agents and ML-Agents-Envs in editable mode
RUN pip install -e /ml-agents/ml-agents-envs
RUN pip install -e /ml-agents/ml-agents
