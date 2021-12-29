from flask import Flask

UPLOAD_FOLDER = 'data'

app = Flask(__name__)
app.secret_key = "asdf43qf43r23trb"
app.config['UPLOAD_FOLDER'] = UPLOAD_FOLDER
app.config['MAX_CONTENT_LENGTH'] = 512 * 1024 * 1024
