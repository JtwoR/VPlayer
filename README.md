# VPlayer
### 基于 NETCORE + ffmpeg + OpenGL + OpenAL 实现的音视频播放器

[![NETCORE](https://img.shields.io/badge/.NETCORE-5-important.svg?style=plastic)]()
[![FFMPEG](https://img.shields.io/badge/FFMPEG-5.x+-brightgreen.svg?style=plastic)](https://github.com/BtbN/FFmpeg-Builds/releases)
[![OPENAL](https://img.shields.io/badge/OpenAL-Last-blue.svg?style=plastic)](https://www.openal.org/downloads/)

---
* ### 说明

    >1、管理员权限cmd，拉取项目:
    >```shell
    >git clone https://github.com/JtwoR/VPlayer.git
    >
    >cd VPlayer
    >```
    >2、[下载ffmpeg编译后的压缩包【ffmpeg-master-latest-win64-gpl-shared.zip】](https://github.com/BtbN/FFmpeg-Builds/releases ) 解压后把bin目录下的文件，复制到项目下的VPlayer项目的FFMPEG文件夹<br>
    >```shell
    >wget -Y on https://github.com/BtbN/FFmpeg-Builds/releases/download/latest/ffmpeg-master-latest-win64-gpl-shared.zip
    >
    >tar xvf ffmpeg-master-latest-win64-gpl-shared.zip
    >
    >xcopy ffmpeg-master-latest-win64-gpl-shared\bin VPlayer\FFMPEG
    >```
    >指令这里使用了代理，如果下载慢可以把指令下的地址复制到浏览器下载<br>【注意：ffmpeg的版本和程序引用的FFmpeg.AutoGen包版本是有关系的，需要自行进行调整】<br>
    >3、编译项目运行<br>
    >```shell
    >dotnet build
    >
    >VPlayer\bin\Debug\net5.0-windows\VPlayer.exe 需要播放的文件路径
    >```
    
---
* ### 效果

![](https://github.com/JtwoR/VPlayer/blob/main/preview.mov)


* ##### 由于刚接触音视频知识，音频处理方面问题会比较大😭😭😭，码率与重采样对各情况的适配未进行处理，有可能会出现噪音的情况，要注意音量。
* ### To do
    - [x] 音视频解码
    - [x] WPF+OpenGL进行视频渲染
    - [x] OpenAL播放音频
    - [x] 音视频同步   
    - [x] 播放条
    - [ ] 音视频适配