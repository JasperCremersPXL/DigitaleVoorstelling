# DigitaleVoorstelling
Digitale Voorstelling Jasper Cremers I-Talent 2021-2022

# Stappenplan
1. Installeer [Docker Engine](https://docs.docker.com/engine/install/)
2. Build de docker container:
  - Linux: run build_container_linux.sh
  - Windows: run ...

![Docker container build](https://github.com/JasperCremersPXL/DigitaleVoorstelling/blob/main/Images/docker_container_build.png)

3. Start de docker cotainer:
  - Linux: run start_container_linux.sh
  - or: run ...
```
cd FixUnityProject/ &&
git submodule update --init --recursive &&
docker build -t unity-robotics:pick-and-place -f docker/Dockerfile .
```
  
4. Om ROS in de container te starten, voer het volgende commando uit:
  - roslaunch niryo_moveit part_3.launch
 
![Docker commando](https://github.com/JasperCremersPXL/DigitaleVoorstelling/blob/main/Images/container-commando.png)
![Docker commando succes](https://github.com/JasperCremersPXL/DigitaleVoorstelling/blob/main/Images/container-commando-succes.png)

5. Run de Unity executable:
  - Linux: I-TalentBuild-Linux/italentbuild.x86_64
  - Windows: I-TalentBuild-Windows/...

6. Enjoy!
