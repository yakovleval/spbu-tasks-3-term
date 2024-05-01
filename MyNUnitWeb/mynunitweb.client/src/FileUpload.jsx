import { useState } from 'react';
import axios from 'axios';
//// Bootstrap CSS
import "bootstrap/dist/css/bootstrap.min.css";
// Bootstrap Bundle JS
import "bootstrap/dist/js/bootstrap.bundle.min";

export default function FileUpload({uploadProgress, setUploadProgress}) {
    const [files, setFiles] = useState([]);
    const [uploading, setUploading] = useState(false)

    function handleMultipleChange(event) {
        setFiles([...event.target.files]);
    }

    function handleMultipleSubmit(event) {
        if (files.length === 0)
            return
        setUploading(true);
        event.preventDefault();
        const url = 'https://localhost:7232/runtests/upload';
        const formData = new FormData();
        files.forEach((file) => {
            formData.append(`files`, file);
        });

        const config = {
            headers: {
                'content-type': 'multipart/form-data',
            },
            onUploadProgress: function (progressEvent) {
                const percentCompleted = Math.round((progressEvent.loaded * 100) / progressEvent.total);
                setUploadProgress(percentCompleted);
            }
        };
        axios.post(url, formData, config)
            .then((response) => {
                console.log(response.data);
                setUploading(false);
            })
            .catch((error) => {
                console.error("Error uploading files: ", error);
                alert(error)
            });
    }

    function uploadButtonContent() {
        if (uploading) {
            return <div className="spinner-border text-primary" role="status">
                <span className="visually-hidden">Loading...</span>
            </div>
        }
        return "Upload"
    }

    return (
        <>
            <div>
                <form onSubmit={handleMultipleSubmit} lang="en">
                    <input type="file" multiple onChange={handleMultipleChange} />
                    <button type="submit">{uploadButtonContent()}</button>
                    <progress value={uploadProgress} max="100"></progress>
                </form>
            </div>
        </>
    );
}
