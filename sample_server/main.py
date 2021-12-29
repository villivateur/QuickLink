import os
import uuid
from pypinyin import lazy_pinyin
from app import app
from flask import request, redirect, jsonify, send_from_directory
from werkzeug.utils import secure_filename

ROOT_URL = "http://127.0.0.1:5000/download"

@app.route('/upload', methods=['POST'])
def upload_file():
    # check if the post request has the file part
    if 'file' not in request.files:
        resp = jsonify({'message' : 'No file part in the request'})
        resp.status_code = 400
        return resp
    file = request.files['file']
    if file.filename == '':
        resp = jsonify({'message' : 'No file selected for uploading'})
        resp.status_code = 400
        return resp
    if file:
        filename = secure_filename(''.join(lazy_pinyin(file.filename)))
        unique_id = str(uuid.uuid4())
        os.makedirs(os.path.join(app.config['UPLOAD_FOLDER'], unique_id))
        file.save(os.path.join(app.config['UPLOAD_FOLDER'], unique_id, filename))
        resp = jsonify({'message' : 'File successfully uploaded', 'url' : f'{ROOT_URL}/{unique_id}/{filename}'})
        resp.status_code = 200
        return resp

@app.route('/download/<path:unique_path>/<path:filename>', methods=['GET', 'POST'])
def download(unique_path, filename):
    uploads = os.path.join(app.root_path, app.config['UPLOAD_FOLDER'], secure_filename(unique_path))
    return send_from_directory(directory=uploads, path=secure_filename(filename))

if __name__ == "__main__":
    app.run()
