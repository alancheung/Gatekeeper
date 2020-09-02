'''
Training Multiple Faces stored on a DataBase:
	==> Each face should have a unique numeric integer ID as 1, 2, 3, etc                       
	==> LBPH computed model will be saved on trainer/ directory. (if it does not exist, pls create one)
	==> for using PIL, install pillow library with "pip install pillow"
    
Based on original code by:
Anirban Kar: https://github.com/thecodacus/Face-Recognition 
Marcelo Rovai - MJRoBot.org @ 21Feb18: https://github.com/Mjrovai/OpenCV-Face-Recognition/blob/master/FacialRecognition/
'''
# ------------------------- DEFINE IMPORTS ---------------------------
from __future__ import print_function
from datetime import datetime
import argparse
import time

import cv2
import numpy as np
import os
from PIL import Image

# ------------------------- DEFINE ARGUMENTS -------------------------
argParser = argparse.ArgumentParser()
argParser.add_argument('--quiet', dest='quiet', action='store_true', help="Disable logging")
argParser.add_argument("-b", "--base-directory", default=".", help="Directory that project files are stored in. Default to currently active directory.")
argParser.add_argument("-f", "--log-file", type=string, default=None, help="Specify file to log to.")
argParser.set_defaults(quiet=False)

args = vars(argParser.parse_args())
quiet = args["quiet"]
baseDirectory = args["base_directory"]
logFileName = args["log_file"]

# ------------------------- DEFINE GLOBALS ---------------------------
# File paths and directory locations
if logFileName is not None:
    logFileName = f"{baseDirectory}/{logFileName}"
datasetFolderPath = f"{baseDirectory}/dataset"
classifierXMLPath = f"{baseDirectory}/haar_frontface_default.xml"
trainingFolderPath = f"{baseDirectory}/trainer"

# ------------------------- DEFINE FUNCTIONS -------------------------
def log(text, displayWhenQuiet = False):
    if displayWhenQuiet or not quiet:
        now = datetime.now().strftime("%H:%M:%S")
        message = f"{now}: {text}"
        if logFileName is not None:
            with open(logFileName, "a") as fout:
                fout.write(f"{message}\n")
        else:
            print(message)

def err(text):
    log(text, True)

def alrt(text):
    log(text, True)

def read_image_files(path):
    '''
    Given an image path, determine the label (id) associated with the image and return the face from the image in grayscale.
    '''

    imagePaths = [os.path.join(path, f) for f in os.listdir(path)]
    
    faceSamples=[]
    ids = []
    for imagePath in imagePaths:
        log(f'Training image at {imagePath}')

        PIL_img = Image.open(imagePath).convert('L') # convert it to grayscale
        img_numpy = np.array(PIL_img, 'uint8')

        id = int(os.path.split(imagePath)[-1].split(".")[1])

        faces = detector.detectMultiScale(img_numpy)
        for (x,y,w,h) in faces:
            faceSamples.append(img_numpy[y:y+h,x:x+w])
            ids.append(id)

    return faceSamples, ids

# ------------------------- DEFINE INITIALIZE ------------------------
log("Initializing...", displayWhenQuiet = True)
log(f"Args: {args}", displayWhenQuiet = True)

recognizer = cv2.face.LBPHFaceRecognizer_create()
detector = cv2.CascadeClassifier(classifierXMLPath);
log("CV2 face libraries initialized!")

if not os.path.exists(trainingFolderPath):
    log(f"'{trainingFolderPath}' not found. Creating!")
    os.makedirs(trainingFolderPath)

# ------------------------- DEFINE RUN -------------------------------
log("Initialized!", displayWhenQuiet = True)
log("Running...", displayWhenQuiet = True)
try:
    log("Run")

    faces, ids = read_image_files(datasetFolderPath)
    log(f"Training {len(faces)} faces for {len(ids)} users. Please wait ...")

    recognizer.train(faces, np.array(ids))
    log("Training completed!")

    # Save the model into trainer/trainer.yml
    # recognizer.save() worked on Mac, but not on Pi
    timeStr = time.strftime("%Y%m%d-%H%M%S")
    trainedFileName = f"{trainingFolderPath}/{timeStr}.yml"
    log(f"Saving to {trainedFileName}...")
    recognizer.write(trainedFileName)

    # Print the numer of faces trained and end program
    log("Completed!")

except KeyboardInterrupt:
    log("KeyboardInterrupt caught! Cleaning up...")

