
'''
Verify facial recognition training results.
'''
# ------------------------- DEFINE IMPORTS ---------------------------
from __future__ import print_function
from datetime import datetime
import time
import argparse

import cv2
import os

# ------------------------- DEFINE ARGUMENTS -------------------------
argParser = argparse.ArgumentParser()
argParser.add_argument('--quiet', dest='quiet', action='store_true', help="Disable logging")
argParser.add_argument("-b", "--base-directory", default=".", help="Directory that project files are stored in. Default to currently active directory.")
argParser.add_argument("-f", "--log-file", default=None, help="Specify file to log to.")
argParser.set_defaults(quiet=False)

args = vars(argParser.parse_args())
quiet = args["quiet"]
baseDirectory = args["base_directory"]
logFileName = args["log_file"]

# ------------------------- DEFINE GLOBALS ---------------------------
# File paths and directory locations
if logFileName is not None:
    logFileName = f"{baseDirectory}/{logFileName}"
classifierXMLPath = cv2.data.haarcascades + 'haarcascade_frontalface_default.xml'

datetimeFormat = "%Y%m%d-%H%M%S"

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

# ------------------------- DEFINE INITIALIZE ------------------------
log("Initializing...", displayWhenQuiet = True)
log(f"Args: {args}", displayWhenQuiet = True)

# Intialize and warmup camera.
camera = cv2.VideoCapture(0)
time.sleep(2)
log("Camera initialized!")
face_detector = cv2.CascadeClassifier(classifierXMLPath);
log("Face detection/classification initialized!")

# ------------------------- DEFINE RUN -------------------------------
log("Initialized!", displayWhenQuiet = True)
log("Running...", displayWhenQuiet = True)
try:
    while True:
        frameRead, frame = camera.read()

        # Rotate image for the keypad.
        frame = cv2.rotate(frame, cv2.ROTATE_90_CLOCKWISE) 
        gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
        faces = face_detector.detectMultiScale(gray, scaleFactor=1.2, minNeighbors=5, minSize=(20, 20))

        for(x,y,w,h) in faces:
            # Draw on the image!
            cv2.rectangle(frame, (x,y), (x+w,y+h), (0,255,0), 2)
        
        # Show output from the camera
        cv2.imshow(f'Feed', frame)

        # See if user wants to quit
        # Press 'ESC' for exiting video
        keyPressed = cv2.waitKey(100) & 0xff
        if keyPressed == 27:
            break
    
except KeyboardInterrupt:
    log("KeyboardInterrupt caught! Cleaning up...")
finally:
    camera.release()
    cv2.destroyAllWindows()

