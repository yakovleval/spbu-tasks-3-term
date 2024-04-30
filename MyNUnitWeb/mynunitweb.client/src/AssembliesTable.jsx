import { useState } from 'react';
import axios from 'axios';

export default function AssembliesTable() {
    const [history, setHistory] = useState([]);
    const [testing, setTesting] = useState(true)

    function runTestsButtonContent() {
        if (testing) {
            return <div className="spinner-border text-primary" role="status">
                <span className="visually-hidden">Loading...</span>
            </div>
        }
        return "Test"
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
            .catch((error) => {
                console.log("Error testing: ", error);
            });
    }

    function status(status) {
        switch (status) {
            case 0: return "PASSED";
            case 1: return "FAILED";
            case 2: return "IGNORED";
        }
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
                            <tr key={assembly.assemblyName} data-bs-toggle="collapse" data-bs-target={"#m" + assembly.assemblyResultId}>
                                <td>
                                    <button type="button">
                                        {assembly.assemblyName}
                                    </button>
                                </td>
                                <td>{assembly.passed}</td>
                                <td>{assembly.failed}</td>
                                <td>{assembly.ignored}</td>
                            </tr>
                            <tr className="collapse" id={"m" + assembly.assemblyResultId} >
                                <td>
                                    <div className="collapse" id={"m" + assembly.assemblyResultId}>
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
        </>
    )
}
