# Gatekeeper
Gatekeeper is a project I started to replace the cheaply made keypad controlling the magnetic lock on my office door. Gatekeeper is designed to run on a RaspberryPi and was originally written in Python. Development on the Python version reached an acceptable point and ultimately Gatekeeper was re-engineered in C# (using Mono).

The goals of the project are to:
- Maintain the same level of functionality as the previous keypad
   - Numerical keypad
   - RFID reader/writer
   - Add new RFID cards on the fly.
- Add new functionality
  - IOT capabilities
  - Weather updates
  - Facial recognition
