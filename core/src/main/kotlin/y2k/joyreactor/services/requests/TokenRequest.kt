package y2k.joyreactor.services.requests

import y2k.joyreactor.common.async.CompletableContinuation
import y2k.joyreactor.common.async.then
import y2k.joyreactor.common.getTextAsync
import y2k.joyreactor.common.http.HttpClient

/**
 * Created by y2k on 4/26/16.
 */
class TokenRequest(private val httpClient: HttpClient) : Function0<CompletableContinuation<String>> {

    override fun invoke(): CompletableContinuation<String> {
        return httpClient
            .getTextAsync("http://joyreactor.cc/donate")
            .then { tokenRegex.find(it)!!.groupValues[1] }
    }

    companion object {

        private val tokenRegex = Regex("var token = '(.+?)'")
    }
}