package com.clevertap.unity;

import android.content.Context;
import android.content.res.AssetManager;
import android.util.Log;

import androidx.annotation.NonNull;

import com.clevertap.android.sdk.CleverTapAPI;
import com.clevertap.android.sdk.inapp.customtemplates.CustomTemplateContext;
import com.clevertap.android.sdk.inapp.customtemplates.CustomTemplateException;
import com.clevertap.android.sdk.inapp.customtemplates.FunctionPresenter;
import com.clevertap.android.sdk.inapp.customtemplates.TemplatePresenter;

import java.io.BufferedReader;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.io.InputStreamReader;
import java.nio.charset.StandardCharsets;

public class CleverTapCustomTemplates {

    private static final String TEMPLATES_DEFINITIONS_ASSETS_FOLDER = "CleverTapSDK/CustomTemplates";

    public static void registerCustomTemplates(Context context) {
        try {
            String[] jsonAssets = context.getAssets().list(TEMPLATES_DEFINITIONS_ASSETS_FOLDER);
            if (jsonAssets == null || jsonAssets.length == 0) {
                return;
            }
            for (String jsonAsset : jsonAssets) {
                String jsonDefinitions = readAsset(context, TEMPLATES_DEFINITIONS_ASSETS_FOLDER + "/" + jsonAsset);
                if (jsonDefinitions != null) {
                    CleverTapAPI.registerCustomInAppTemplates(jsonDefinitions, createTemplatePresenter(), createFunctionPresenter());
                }
            }
        } catch (IOException ioException) {
            Log.e(CleverTapUnityPlugin.LOG_TAG, "Could not read template definitions folder. Custom templates will not work.", ioException);
        }
    }

    private static String readAsset(Context context, String asset) {
        AssetManager assetManager = context.getAssets();

        try (BufferedReader reader = new BufferedReader(new InputStreamReader(assetManager.open(asset), StandardCharsets.UTF_8))) {
            StringBuilder stringBuilder = new StringBuilder();
            String line = reader.readLine();
            while (line != null) {
                stringBuilder.append(line);
                line = reader.readLine();
            }
            return stringBuilder.toString();
        } catch (FileNotFoundException fnfException) {
            throw new CustomTemplateException("Template definitions asset not found", fnfException);
        } catch (IOException ioException) {
            Log.e(CleverTapUnityPlugin.LOG_TAG, "Could not read template definitions asset. Custom templates will not work.", ioException);
            return null;
        }
    }

    private static TemplatePresenter createTemplatePresenter() {
        return new TemplatePresenter() {

            @Override
            public void onClose(@NonNull CustomTemplateContext.TemplateContext templateContext) {
                CleverTapMessageSender.getInstance().send(CleverTapUnityCallback.CLEVERTAP_CUSTOM_TEMPLATE_CLOSE, templateContext.getTemplateName());
            }

            @Override
            public void onPresent(@NonNull CustomTemplateContext.TemplateContext templateContext) {
                CleverTapMessageSender.getInstance().send(CleverTapUnityCallback.CLEVERTAP_CUSTOM_TEMPLATE_PRESENT, templateContext.getTemplateName());
            }
        };
    }

    private static FunctionPresenter createFunctionPresenter() {
        return functionContext -> CleverTapMessageSender.getInstance().send(CleverTapUnityCallback.CLEVERTAP_CUSTOM_FUNCTION_PRESENT, functionContext.getTemplateName());
    }
}
