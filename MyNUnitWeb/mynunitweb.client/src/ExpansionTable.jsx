function ExpansionTable(classes) {
    return <table className="table">
        {classes === undefined ? "" : classes.map(testclass =>
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
                            </tr>
                        </thead>
                        <tbody>
                            {testclass.methods.map(method =>
                                <tr key={"method" + method.methodName}>
                                    <td>{method.methodName}</td>
                                    <td>{method.status}</td>
                                    <td>{method.reason}</td>
                                </tr>
                            )}
                        </tbody>
                    </table>
                </table>
            </>
        )}
    </table>
}

export default ExpansionTable