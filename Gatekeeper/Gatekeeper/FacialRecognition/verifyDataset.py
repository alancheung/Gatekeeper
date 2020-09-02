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
classifierXMLPath = f"{baseDirectory}/haar_frontface_default.xml"
guestListDirectory = f"{baseDirectory}/trainer"
guestListPath = None

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

def findLatestGuestList(path):
    trainingYmls = os.listdir(path)

    dates = [time.strptime(fileName.split(".")[0], datetimeFormat) for fileName in trainingYmls]
    maxDate = max(dates)

    return f"{path}/{time.strftime(datetimeFormat, maxDate)}.yml"

# ------------------------- DEFINE INITIALIZE ------------------------
log("Initializing...", displayWhenQuiet = True)
log(f"Args: {args}", displayWhenQuiet = True)

# Intialize and warmup camera.
camera = cv2.VideoCapture(0)
time.sleep(2)
log("Camera initialized!")

guestListPath = findLatestGuestList(guestListDirectory)
log(f"Found latest guest list at '{guestListPath}'")

face_detector = cv2.CascadeClassifier(classifierXMLPath);
doorman = cv2.face.LBPHFaceRecognizer_create()
doorman.read(guestListPath)
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
            # Check if confidence is less them 100 ==> "0" is perfect match 
            id, confidence = doorman.predict(gray[y:y+h,x:x+w])
            if (confidence >= 100):
                id = "unknown"

            # Make pretty percentage
            confidence = f"{round(100 - confidence)}%"

            # Draw on the image!
            cv2.rectangle(frame, (x,y), (x+w,y+h), (0,255,0), 2)
            cv2.putText(frame, str(id), (x+5,y-5), cv2.FONT_HERSHEY_SIMPLEX, 1, (255,255,255), 2)
            cv2.putText(frame, str(confidence), (x+5,y+h-5), cv2.FONT_HERSHEY_SIMPLEX, 1, (255,255,0), 1)
        
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

