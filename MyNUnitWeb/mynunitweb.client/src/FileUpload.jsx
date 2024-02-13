import { useState } from 'react';
import axios from 'axios';
//// Bootstrap CSS
import "bootstrap/dist/css/bootstrap.min.css";
// Bootstrap Bundle JS
import "bootstrap/dist/js/bootstrap.bundle.min";

function FileUpload() {
    const [files, setFiles] = useState([]);
    const [uploadProgress, setUploadProgress] = useState(0);
    const [history, setHistory] = useState([]);
    const [uploading, setUploading] = useState(false)
    const [testing, setTesting] = useState(true)


    function handleMultipleChange(event) {
        setFiles([...event.target.files]);
    }

    function handleMultipleSubmit(event) {
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
                //setUploadedFiles(response.data.files);
            })
            .catch((error) => {
                console.error("Error uploading files: ", error);
            });
    }

    async function handleRunTestsSubmit(event) {
        setTesting(true);
        event.preventDefault();
        const url = 'https://localhost:7232/runtests/GetTestResults';
        axios.get(url)
            .then((response) => {
                console.log(response);
                setHistory(response.data.concat(history));
                setTesting(false);
            })
            .catch ((error) => {
                console.log("Error testing: ", error);
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

    function runTestsButtonContent() {
        if (testing) {
            return <div className="spinner-border text-primary" role="status">
                <span className="visually-hidden">Loading...</span>
            </div>
        }
        return "Test"
    }

    axios.get('https://localhost:7232/runtests/gethistory')
        .then((response) => {
            console.log(response);
            setHistory(response.data);
            setTesting(false);
        })
        .catch((error) => {
            console.log("Error testing: ", error);
        });

    return (
        <>
            <div>
                <form onSubmit={handleMultipleSubmit} lang="en">
                    <input type="file" multiple onChange={handleMultipleChange} />
                    <button type="submit">{uploadButtonContent()}</button>
                    <progress value={uploadProgress} max="100"></progress>
                </form>
                <form onSubmit={handleRunTestsSubmit}>
                    <button type="submit">{runTestsButtonContent()}</button>
                </form>
                <table className="table">
                    <thead>
                        <tr>
                            <th scope="col">Assembly name</th>
                            <th scope="col">Passed</th>
                            <th scope="col">Failed</th>
                            <th scope="col">Ignored</th>
                        </tr>
                    </thead>
                    <tbody>
                        {history.map(assembly =>
                            <>
                                <tr key={assembly.assemblyName} data-bs-toggle="collapse" data-bs-target={"#m" + assembly.assemblyId}>
                                    <td>
                                        <button type="button">
                                            {assembly.assemblyName}
                                        </button>
                                    </td>
                                    <td>{assembly.passed}</td>
                                    <td>{assembly.failed}</td>
                                    <td>{assembly.ignored}</td>
                                </tr>
                                <tr className="collapse" id={"m" + assembly.assemblyId} >
                                    <td>
                                        <div className="collapse" id={"m" + assembly.assemblyId}>
                                            <table className="table">
                                                {assembly.classResults.map(testclass =>
                                                    testclass.reason !== null ?
                                                    `${testclass.className} -- FAILED, reason: ${testclass.reason}` :
                                                    <>
                                                        <table className="table">
                                                            <tr>
                                                                <th>{testclass.className}</th>
                                                            </tr>
                                                            <table className="table">
                                                                <thead>
                                                                    <tr>
                                                                        <th>Method name</th>
                                                                        <th>Status</th>
                                                                        <th>Reason</th>
                                                                        <th>Expected</th>
                                                                        <th>Was</th>
                                                                    </tr>
                                                                </thead>
                                                                <tbody>
                                                                    {testclass.methodResults === undefined ? "undefined" : testclass.methodResults.map(method =>
                                                                        <tr key={"method" + method.met}>
                                                                            <td>{method.methodName}</td>
                                                                            <td>{status(method.status)}</td>
                                                                            <td>{method.reason}</td>
                                                                            <td>{method.expected}</td>
                                                                            <td>{method.was}</td>
                                                                        </tr>
                                                                    )}
                                                                </tbody>
                                                            </table>
                                                        </table>
                                                    </>
                                                )}
                                            </table>
                                        </div>
                                    </td>
                                </tr>
                            </>
                        )}
                    </tbody>
                </table>
            </div>
        </>
    );
}

function status(status) {
    switch (status) {
        case 0: return "PASSED";
        case 1: return "FAILED";
        case 2: return "IGNORED";
    }
}

export default FileUpload