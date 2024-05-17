import sys
import base64
import requests
import json
import os

# Retrieve command line arguments
image_path = sys.argv[1]  # First argument: image path
prompt_text = sys.argv[2]  # Second argument: prompt

# Function to encode the image
def encode_image(path):
    with open(path, "rb") as image_file:
        return base64.b64encode(image_file.read()).decode('utf-8')

# OpenAI API Key
api_key = os.getenv('OPENAI_KEY')
if not api_key:
    raise ValueError("No API key provided. Set the OPENAI_KEY environment variable.")

# Encode the image
base64_image = encode_image(image_path)

# Set up headers and payload
headers = {
    "Content-Type": "application/json",
    "Authorization": f"Bearer {api_key}"
}

payload = {
    "model": "gpt-4o",
    "messages": [
        {
            "role": "user",
            "content": [
                {"type": "text", "text": prompt_text},
                {"type": "image_url", "image_url": {"url": f"data:image/jpeg;base64,{base64_image}"}}
            ]
        }
    ],
    "max_tokens": 300
}

# Send request to OpenAI API
response = requests.post("https://api.openai.com/v1/chat/completions", headers=headers, json=payload)

# Print the response to stdout (to be captured by Unity)
print(json.dumps(response.json()))
