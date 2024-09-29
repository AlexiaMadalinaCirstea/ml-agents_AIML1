# Unity ML-Agents Toolkit Fork of Group 1

[![docs badge](https://img.shields.io/badge/docs-reference-blue.svg)](https://github.com/Unity-Technologies/ml-agents/tree/release_21_docs/docs/)

[![license badge](https://img.shields.io/badge/license-Apache--2.0-green.svg)](../LICENSE.md)

([latest release](https://github.com/Unity-Technologies/ml-agents/releases/tag/latest_release))  
([all releases](https://github.com/Unity-Technologies/ml-agents/releases))

# Project Overview
This project explores the implementation and analysis of state-of-the-art Deep Reinforcement Learning (DRL) algorithms to control agents in a 3D real-time video game environment using the Unity Game Engine and Unity ML-Agents Toolkit. The goal is to evaluate and improve the performance of machine learning models that govern agent behavior in various interactive game environments.

# Objectives
1. Apply and analyze DRL algorithms: Train agents in real-time environments and monitor their performance.
2. Develop new sensor inputs: Implement custom sensors for the agent to interact with their environments more realistically.
3. Performance analysis: Profile and analyze the computational performance of these models across different scenarios.

# Clone of the Fork:
git clone https://github.com/AlexiaMadalinaCirstea/ml-agents_AIML1

# Opening the Project:
1. Open Unity Hub and go to Projects
2. Click on Add, then select Add project from disk
3. Navigate to forked repository folder in your computer
4. Open folder, add the Project folder as a Unity project
5. Double click and open this project in Unity

# NOTE! First you need to fork and then clone the repository! Not the other way around!

# Required Software:
Unity Game Engine (Free version: Unity Personal or Student License)
ML-Agents Toolkit (Branch: fix-numpy-release-21-branch)
Git for version control
Python 3.10.x with required virtual environments for ML-Agents

# References:
Original fork from Dennis Soemers: https://github.com/DennisSoemers/ml-agents/tree/fix-numpy-release-21-branch

# Common Installation Issues and Solutions
During the installation and setup of Unity ML-Agents Toolkit, you might encounter several common issues. Below are some of the most frequent problems, and platform-specific solutions for Anaconda, Windows (CMD), macOS, and Linux.

# 1. Python Version Compatibility Issues
ML-Agents requires specific versions of Python (3.10.x). If you have multiple Python versions installed, you might encounter conflicts during installation.

# Solution (Anaconda):
Create a new Anaconda environment with the correct Python version:
conda create --name ml-agents python=3.10
Activate the environment:
conda activate ml-agents
Install the required packages:
pip install -r requirements.txt

# Solution (Windows CMD):
Use the following command to create a virtual environment:
python -m venv ml-agents-env
Activate the environment:
ml-agents-env\Scripts\activate
Install the required dependencies:
pip install -r requirements.txt

# Solution (macOS/Linux):
Create a virtual environment with Python:
python3 -m venv ml-agents-env
Activate the environment:
source ml-agents-env/bin/activate
Install the required dependencies:
pip install -r requirements.txt

# 2. Unity Hub or Editor Version Mismatch
You may face compatibility issues if the version of Unity Hub or the Unity Editor you’re using is incompatible with the ML-Agents Toolkit.

Solution: Ensure you’re using the correct Unity version (Unity 2022.3 LTS or newer). You can manage different Unity versions via Unity Hub.

# 3. ML-Agents Installation Fails Due to Missing Dependencies
If the ML-Agents installation fails, you might be missing essential dependencies.

# Solution (Anaconda):
Clone the correct fork and branch of ML-Agents:
git clone https://github.com/AlexiaMadalinaCirstea/ml-agents_AIML1
Install the necessary dependencies in your Anaconda environment:
pip install -r requirements.txt
If you encounter issues with `numpy`, install the specific version:
pip install numpy==1.21.0

# Solution (Windows CMD, macOS, Linux):
Clone the correct branch and install dependencies:
git clone https://github.com/AlexiaMadalinaCirstea/ml-agents_AIML1
pip install -r requirements.txt    
If the installation fails due to `numpy` errors:
pip install numpy==1.21.0

# 4. Unity Editor Crashes on Import
Sometimes the Unity Editor crashes while importing assets or the ML-Agents Toolkit. This can be due to a lack of system resources or corrupted files.

Solution: Clear the Unity cache by deleting the Library folder in your project directory. Unity will recreate this folder on the next launch.

# 5. Python Dependencies Conflict
You may encounter errors due to conflicting versions of Python libraries.

# Solution (Anaconda):
Always use a separate Anaconda environment for each project to avoid dependency conflicts:
conda create --name ml-agents python=3.10
Activate the environment:
conda activate ml-agents
Check installed packages with:
conda list

# Solution (Windows CMD, macOS, Linux):
Create separate virtual environments for different projects:
python3 -m venv ml-agents-env
Activate the environment:
source ml-agents-env/bin/activate
Check installed packages with:
pip freeze

# 6. C# or Visual Studio Issues in Unity
If you face errors related to C# scripts or Visual Studio integration in Unity, the problem may be related to the incorrect installation of Visual Studio components.

Solution: Ensure you have installed the necessary C# development tools in Visual Studio (Game Development with Unity, .NET Desktop Development).
Rebuild your Unity project via Assets > Open C# Project.

# 7. Virtual Environment Activation Issues (Windows)
On Windows, sometimes the virtual environment’s activate script doesn’t work due to execution policies.

Solution: Enable script execution by running the following command in PowerShell as Administrator:
Set-ExecutionPolicy RemoteSigned -Scope CurrentUser

# 8. Unity ML-Agents Version Conflicts
Using an outdated or unsupported version of ML-Agents can result in errors.

Solution: lways use the specific branch of ML-Agents mentioned in the setup guide:
git clone https://github.com/AlexiaMadalinaCirstea/ml-agents_AIML1
