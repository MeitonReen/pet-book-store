

export default class StatusCodeService {
	//Выполняется первый подходящий обработчик на один объект
	/*
	.if([<codes>], response, () => {...})
	.if([], error, () => {...})
	.if([], response, () => {...})
	*/
	/*Пример:
		new StatusCodeService()
			.if([401], response, () => setRedirectToAuthentication(true))
			.if([200, 201], response, () => {...})
			.if([400], response, () => {...})
			.if([404], response, () => {...})
			.if([500], response, () => {...})
			.if([], error, () => {...})
			.if([], response, () => {...})
*/

	constructor() {
		this.selectedDefaultHandler = null;
		StatusCodeService.prototype.navDefaultHandlers = {
			400: this.if400,
			401: this.if401,
			404: this.if404,
			500: this.if500,
			200: this.if200,
			201: this.if201,
			204: this.if204
		};
	}
	handle(toDoCallback) {
		if (this.selectedDefaultHandler) {
			this.selectedDefaultHandler();
		}
		if (toDoCallback) {
			toDoCallback();
		}
		return;
	}
	if400(response) {
		console.error(response.status + ": " +
			response.body?.message);
		return;
	}
	if401(response) {
		console.error(response.status + ": " +
			response.body?.message);
		return;
	}
	if404(response) {
		console.error(response.status + ": " +
			response.body?.message);
		return;
	}
	if500(response) {
		console.error(response.status);
		return;
	}
	if200(response) {
		return;
	}
	if201(response) {
		return;
	}
	if204(response) {
		return;
	}
	if(statusCodeArray, response, toDoCallback) {
		if (!this.selectedHandler && Array.isArray(statusCodeArray)) {
			if (statusCodeArray.length === 0) {
				if (!response?.status) {//.if([], error, () => {...})
					const error = response;
					this.selectedHandler = () => this.ifUnknownError(error);
				} else {//.if([], response, () => {...})
					this.selectedHandler = () => this.ifUnknownResult(response);
				}
			} else if (response?.status && this.checkContains(statusCodeArray,
				response.status)) {//.if([<codes>], response, () => {...})

				this.selectedDefaultHandler = () => this.navDefaultHandlers[
					response.status](response);
			}
			else {
				return this;
			}
			this.handle(toDoCallback);
		}
		return this;
	}
	ifUnknownError(error) {
		console.error("Unknown error: ", error);
		return;
	}
	ifUnknownResult(response) {
		console.error("Unknown result: ", response);
		return;
	}
	checkContains(statusCodeArray, statusCode) {
		return statusCodeArray.includes(statusCode);
	}
}