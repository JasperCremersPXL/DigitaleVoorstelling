cd UnityProject/ &&
git submodule update --init --recursive &&
docker build -t unity-robotics:pick-and-place -f docker/Dockerfile .
