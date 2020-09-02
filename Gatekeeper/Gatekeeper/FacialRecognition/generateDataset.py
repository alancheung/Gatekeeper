'''
Capture multiple Faces from multiple users to be stored on a DataBase (dataset directory)
	==> Faces will be stored on a directory: dataset/ (if does not exist, pls create one)
	==> Each face will have a unique numeric integer ID as 1, 2, 3, etc                       
Based on original code by:
Anirban Kar: https://github.com/thecodacus/Face-Recognition 
Marcelo Rovai - MJRoBot.org @ 21Feb18: https://github.com/Mjrovai/OpenCV-Face-Recognition/blob/master/FacialRecognition/
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
argParser.add_argument("-c", "--image-count", type=int, default=10, help="Number of images to take per expression.")
argParser.set_defaults(quiet=False)

args = vars(argParser.parse_args())
quiet = args["quiet"]
baseDirectory = args["base_directory"]
logFileName = args["log_file"]
imageCount = args["image_count"]

# ------------------------- DEFINE GLOBALS ---------------------------
# File paths and directory locations
if logFileName is not None:
    logFileName = f"{baseDirectory}/{logFileName}"
classifierXMLPath = f"{baseDirectory}/haar_frontface_default.xml"
datasetFolderPath = f"{baseDirectory}/dataset"

expressions = ["look directly at the camera.", "smile.", "look left.", "look right.", "look up.", "look down."]
totalImages = len(expressions) * imageCount

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

def display_text(frame, message):
    cv2.putText(frame, message, (10, 20), cv2.FONT_HERSHEY_SIMPLEX, 0.5, (0, 0, 255), 2)

# ------------------------- DEFINE INITIALIZE ------------------------
log("Initializing...", displayWhenQuiet = True)
log(f"Args: {args}", displayWhenQuiet = True)

if not os.path.exists(datasetFolderPath):
    log(f"'{datasetFolderPath}' not found. Creating!")
    os.makedirs(datasetFolderPath)

# Intialize and warmup camera.
camera = cv2.VideoCapture(0)
time.sleep(2)
log("Camera initialized!")

face_detector = cv2.CascadeClassifier(classifierXMLPath);
log("Face detector initialized!")

# ------------------------- DEFINE RUN -------------------------------
log("Initialized!", displayWhenQuiet = True)
log("Running...", displayWhenQuiet = True)
try:
    face_id = input("Enter numerical id for user: ")
    while not str(face_id).isnumeric:
        err("ID should be numerical!")
        face_id = input("Enter numerical id for user: ")

    log(f"Initializing face capture. You will be asked to take a total of {totalImages} pictures in {len(expressions)} positions.")
    count = 0

    for expression in expressions:
        log(f"Please {expression}.")
        input("Press any key to continue...")

        while True:
            frameRead, frame = camera.read()

            # Rotate image for the keypad.
            frame = cv2.rotate(frame, cv2.ROTATE_90_CLOCKWISE) 
            gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
            faces = face_detector.detectMultiScale(gray, scaleFactor=1.2, minNeighbors=5, minSize=(20, 20))

            # Only use frame if it has 1 face in it.
            if len(faces) != 1:
                display_text(frame, f"Found {len(faces)} faces. There should only be one!")
                cv2.imshow(f'Feed', frame)
                continue

            # Save the captured image into the datasets folder
            (x, y, w, h) = faces[0]
            cv2.imwrite(f"datasetFolderPath/user." + str(face_id) + '.' + str(count) + ".jpg", gray[y:y+h,x:x+w])
            count += 1
        
            # Show output from the camera
            display_text(frame, f"Looking good! ({count} of {totalImages})")
            cv2.imshow(f'Feed', frame)

            # See if user wants to quit
            # Press 'ESC' for exiting video
            keyPressed = cv2.waitKey(100) & 0xff
            if keyPressed == 27:
                break
            elif count >= imageCount:
                 break

except KeyboardInterrupt:
    log("KeyboardInterrupt caught! Cleaning up...")
finally:
    camera.release()
    cv2.destroyAllWindows()

