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

<h2>Required Software</h2>
<ul>
    <li><strong>Unity Game Engine</strong> (Free version: Unity Personal or Student License, version 2022.3 LTS or newer)</li>
    <li><strong>ML-Agents Toolkit</strong> (Branch: <code>fix-numpy-release-21-branch</code>)</li>
    <li><strong>Git</strong> for version control</li>
    <li><strong>Python 3.10.x</strong> with required virtual environments for ML-Agents</li>
    <li><strong>Visual Studio Code</strong> (for C# development in Unity)</li>
</ul>

<h2>1. Install Unity 2022.3 or Later</h2>
<p><a href="https://unity3d.com/get-unity/download" target="_blank">Download</a> and install Unity. We recommend using <strong>Unity Hub</strong> to manage multiple Unity versions. Ensure that you use <strong>Unity 2022.3 LTS</strong> or newer, as earlier versions may not be fully compatible with the latest ML-Agents toolkit.</p>

<h2>2. Install Python 3.10.x</h2>
<p>We recommend <a href="https://www.python.org/downloads/" target="_blank">installing Python 3.10.12</a>. If you are using Windows, install the <strong>x86-64</strong> version. We also recommend using <strong>conda</strong> or <strong>virtualenv</strong> to manage Python environments.</p>

<h3>Conda Python Setup</h3>
<p>If you're using Conda to manage Python environments, run the following commands:</p>
<pre><code>conda create --name ml-agents python=3.10.12
conda activate ml-agents
</code></pre>

<h3>Pip Setup (Windows CMD)</h3>
<pre><code>python -m venv ml-agents-env
ml-agents-env\Scripts\activate
python -m pip install -r requirements.txt
</code></pre>

<h3>Brew Setup (macOS/Linux)</h3>
<pre><code>brew install python
python3 -m venv ml-agents-env
source ml-agents-env/bin/activate
python -m pip install -r requirements.txt
</code></pre>

<h2>3. Clone the ML-Agents Toolkit Fork</h2>
<p>To clone the repository and use our custom version of ML-Agents:</p>
<pre><code>git clone https://github.com/AlexiaMadalinaCirstea/ml-agents_AIML1
</code></pre>
<p>Navigate to the repository folder and install the required packages:</p>
<pre><code>pip install -r requirements.txt
</code></pre>

<h2>4. Install the <code>com.unity.ml-agents</code> Unity Package</h2>
<p>The ML-Agents C# SDK is a Unity Package. Install it through the Unity <strong>Package Manager</strong> by adding it from disk:</p>
<ol>
    <li>Navigate to <code>Window -> Package Manager</code>.</li>
    <li>Click the <strong>+</strong> button and select <strong>Add package from disk</strong>.</li>
    <li>Choose the <code>com.unity.ml-agents</code> folder from the cloned repository.</li>
    <li>Select the <code>package.json</code> file and proceed with the installation.</li>
</ol>

<p>If the package doesn't appear, make sure that 'Preview Packages' is enabled in the <strong>Advanced</strong> settings of the Unity Package Manager.</p>

<h2>5. Install Python Packages: <code>mlagents</code> and <code>mlagents_envs</code></h2>
<p>These Python packages handle the machine learning algorithms and API communications between Python and Unity.</p>
<pre><code>python -m pip install mlagents
python -m pip install mlagents_envs
</code></pre>
<p>Alternatively, if you cloned the repository, run the following command:</p>
<pre><code>cd /path/to/ml-agents
python -m pip install ./ml-agents-envs
python -m pip install ./ml-agents
</code></pre>

<h2>Common Installation Issues and Solutions</h2>
<p>During the installation and setup of Unity ML-Agents Toolkit, you might encounter several common issues. Below are some platform-specific solutions for <strong>Anaconda</strong>, <strong>Windows (CMD)</strong>, <strong>macOS</strong>, and <strong>Linux</strong>.</p>

<h3>1. Python Version Compatibility Issues</h3>
<p>ML-Agents requires specific versions of Python (3.10.x). You might encounter conflicts if multiple Python versions are installed.</p>

<h4>Solution (Anaconda)</h4>
<pre><code>conda create --name ml-agents python=3.10
conda activate ml-agents
pip install -r requirements.txt
</code></pre>

<h4>Solution (Windows CMD)</h4>
<pre><code>python -m venv ml-agents-env
ml-agents-env\Scripts\activate
pip install -r requirements.txt
</code></pre>

<h4>Solution (macOS/Linux)</h4>
<pre><code>python3 -m venv ml-agents-env
source ml-agents-env/bin/activate
pip install -r requirements.txt
</code></pre>

<h3>2. Unity Hub or Editor Version Mismatch</h3>
<p>Ensure you’re using <strong>Unity 2022.3 LTS</strong> or newer. Manage Unity versions via <strong>Unity Hub</strong> to avoid compatibility issues.</p>

<h3>3. ML-Agents Installation Fails Due to Missing Dependencies</h3>
<p>Clone the correct fork and install dependencies:</p>
<pre><code>git clone https://github.com/AlexiaMadalinaCirstea/ml-agents_AIML1
pip install -r requirements.txt
</code></pre>
<p>If there are issues with <code>numpy</code>, use this command:</p>
<pre><code>pip install numpy==1.21.0
</code></pre>

<h3>4. Unity Editor Crashes on Import</h3>
<p>If Unity crashes, try clearing the Unity cache by deleting the <code>Library</code> folder in your project directory. Unity will regenerate it on the next launch.</p>

<h3>5. Python Dependency Conflicts</h3>
<p>Always use a separate virtual environment for each project to avoid conflicts.</p>

<h4>Solution (Anaconda)</h4>
<pre><code>conda create --name ml-agents python=3.10
conda activate ml-agents
conda list
</code></pre>

<h4>Solution (Windows CMD, macOS, Linux)</h4>
<pre><code>python3 -m venv ml-agents-env
source ml-agents-env/bin/activate
pip freeze
</code></pre>

<h3>6. C# or Visual Studio Issues in Unity</h3>
<p>Ensure you have the necessary <strong>C# development tools</strong> installed in Visual Studio (e.g., <strong>Game Development with Unity</strong>, <strong>.NET Desktop Development</strong>). Rebuild your Unity project via <code>Assets &gt; Open C# Project</code>.</p>

<h3>7. Virtual Environment Activation Issues (Windows)</h3>
<p>Run this PowerShell command as an administrator:</p>
<pre><code>Set-ExecutionPolicy RemoteSigned -Scope CurrentUser
</code></pre>

<h3>8. ML-Agents Version Conflicts</h3>
<p>Always use the specific branch of ML-Agents mentioned in the setup guide. Clone the repository and switch to the correct branch:</p>
<pre><code>git clone https://github.com/AlexiaMadalinaCirstea/ml-agents_AIML1
</code></pre>

<h2>References</h2>
<p>Original fork from Dennis Soemers: <a href="https://github.com/DennisSoemers/ml-agents/tree/fix-numpy-release-21-branch" target="_blank">https://github.com/DennisSoemers/ml-agents/tree/fix-numpy-release-21-branch</a></p>

</body>
</html>
