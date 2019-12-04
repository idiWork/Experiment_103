# Experiment #103 - Facial Recognition Access Control
#### Using Cognitive Services and Internet of Things for Facial Recognition Access Control.

------------

##### Project Date:
- November-December 2019

##### Services:
- Face API 
- Text to Speech API 
- Windows 10 IoT Core


##### Technology used:
- Cognitive Services
- Machine Learning

------------

## About 
The Experiment #103 researches about the possibilities of applying Cognitive Services to create an access control system only managed by an Artificial Intelligence by giving it the capabilities of speaking human languages and recognizing human faces.

## Idea
The project tries to simulate an automatic access control system with facial recognition. The application will tell the visitor if he/she is or is not in a white list in order to open and welcome him/her or to keep the door closed to strangers. The idea is based on an old project from huckster.io created by Ethan Kusters and Masato Sudo and published in 2016.

## Utility
For this simulation we will build a smart box. This box will contain an item hidden behind a little door. The door will be locked until the application recognizes a face included in the white list. If the Artificial Intelligence recognizes the visitor the door will automatically open, showing up the item. This way, the owners of this system only have to take care of who take part of the white list by enrolling the different visitors.

## Process
First, we are going to create a Windows Universal Platform application with C Sharp and .Net Core. This application will be our user interface. After that, we will connect a Webcam to interact with the Cognitive Service Vision Face API. Then, we will to ensemble all the electronic parts, a LED light, a button and a servo motor. We will install Windows 10 IoT Core on a Raspberry Pi 2 model B GPIO device to run the application on it. Also, we will build our 3D printed smart box to contain the entire project and the electronics. Finally, we will plug in a speaker to hear the machine speaking thanks to the Cognitive Service Text to Speech API.

## Advantages
The principal advantage of using Cognitive Services in this simulation is the possibility of identifying people quickly and accurately. The Artificial Intelligence can work for us 24 hours a day, 7 days a week without any interruption. This way a company will save money and resources and will be very much efficient controlling the accesses to a building for example. In addition, with the capability to speak we can give the machine a more human appearance.