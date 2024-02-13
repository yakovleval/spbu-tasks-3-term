import { useEffect, useState } from 'react';
import './App.css';
import FileUpload from './FileUpload';

function App() {
    //const [forecasts, setForecasts] = useState();

    //useEffect(() => {
    //    populateWeatherData();
    //}, []);

    //const contents = forecasts === undefined
    //    ? <p><em>Loading... Please refresh once the ASP.NET backend has started. See <a href="https://aka.ms/jspsintegrationreact">https://aka.ms/jspsintegrationreact</a> for more details.</em></p>
    //    : forecasts[0].assemblyName;
        //: <table className="table table-striped" aria-labelledby="tabelLabel">
        //    <thead>
        //        <tr>
        //            <th>Date</th>
        //            <th>Temp. (C)</th>
        //            <th>Temp. (F)</th>
        //            <th>Summary</th>
        //        </tr>
        //    </thead>
        //    <tbody>
        //        {forecasts.map(forecast =>
        //            <tr key={forecast.date}>
        //                <td>{forecast.date}</td>
        //                <td>{forecast.temperatureC}</td>
        //                <td>{forecast.temperatureF}</td>
        //                <td>{forecast.summary}</td>
        //            </tr>
        //        )}
        //    </tbody>
        //</table>;

    return (
        <FileUpload />
    );
    
    //async function populateWeatherData() {
    //    //axios.get('https://localhost:7232/runtests')
    //    //    .then((response) => setForecasts(response))
    //    //    .catch((error) => {
    //    //        console.error("Error getting response: ", error);
    //    //    });
    //    const response = await fetch('https://localhost:7232/runtests');
    //    const data = await response.json();
    //    setForecasts(data);
    //}
}

export default App;