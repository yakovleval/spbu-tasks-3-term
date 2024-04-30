export default function ClassesTable({ classResults }) {
    function status(status) {
        switch (status) {
            case 0: return "PASSED";
            case 1: return "FAILED";
            case 2: return "IGNORED";
        }
    }

    return (
        <table className="table">
            {classResults.map(testclass =>
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
                                        <tr key={method.MethodResultId}>
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
    )
}
