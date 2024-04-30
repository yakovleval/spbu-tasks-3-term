import './App.css';
import { useState } from 'react';
import AssembliesTable from './AssembliesTable';
import FileUpload from './FileUpload';

export default function App() {
    const [uploadProgress, setUploadProgress] = useState(0);
    return (
        <>
            <FileUpload
                uploadProgress={uploadProgress}
                setUploadProgress={setUploadProgress}
            />
            <AssembliesTable
                setUploadProgress={setUploadProgress}
            />
        </>
    );
}
