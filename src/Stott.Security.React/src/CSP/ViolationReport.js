import React, { useState, useEffect } from "react";
import axios from 'axios';
import ConvertCspViolation from "./ConvertCspViolation";
import Moment from "react-moment";
import { Container, Alert } from "react-bootstrap";
import SourceFilter from "./SourceFilter";

const ViolationReport = (props) => {

    const [cspViolations, setcspViolations] = useState([])

    useEffect(() => {
        getCspViolations('', '')
    },[])

    const getCspViolations = async (sourceQuery, directiveQuery) => {
        await axios.get(process.env.REACT_APP_VIOLATIONREPORT_LIST_URL, { params: { source: sourceQuery, directive: directiveQuery } })
            .then((response) => {
                setcspViolations(response.data);
            },
            () => {
                handleShowFailureToast('Error', 'Failed to load the Content Security Policy violation history.');
            });
    }

    const handleSourceFilterChange = (source, directive) => getCspViolations(source, directive);

    const handleShowFailureToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(false, title, description);

    const renderViolationList = () => {
        return cspViolations && cspViolations.map(cspViolation => {
            const { key, source, sanitizedSource, sourceSuggestions, directive, directiveSuggestions, violations, lastViolated } = cspViolation
            return (
                <tr key={key}>
                    <td>{source}</td>
                    <td>{directive}</td>
                    <td>{violations}</td>
                    <td><Moment format="YYYY-MM-DD HH:mm:ss">{lastViolated}</Moment></td>
                    <td>
                        <ConvertCspViolation
                            cspViolationUrl={sanitizedSource}
                            cspViolationDirective={directive}
                            cspSourceSuggestions={sourceSuggestions}
                            cspDirectiveSuggestions={directiveSuggestions}
                            showToastNotificationEvent={props.showToastNotificationEvent}></ConvertCspViolation>
                    </td>
                </tr>
            )
        })
    }

    return(
        <div>
            <Container className="mb-3">
                <Alert variant='primary'>Please note that new violations of the Content Security Policy (CSP) may take several minutes to appear depending on the browser.</Alert>
            </Container>
            <Container fluid className="mb-3">
                <SourceFilter onSourceFilterUpdate={handleSourceFilterChange}></SourceFilter>
            </Container>
            <table className='table table-striped'>
                <thead>
                    <tr>
                        <th>Source</th>
                        <th>Directive</th>
                        <th>Violations</th>
                        <th>Last Violated</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {renderViolationList()}
                </tbody>
            </table>
        </div>
    )

}

export default ViolationReport;